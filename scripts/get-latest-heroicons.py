from glob import iglob
import os
import urllib.request
import json
from pathlib import Path
import cgi
import shutil
import tarfile


def main():
    # move to project root
    os.chdir("..")

    # clean up prior run, if required
    if Path("tmp").exists():
        print("Cleaning up existing tmp files...")
        shutil.rmtree("tmp")

    # get latest release info
    print("Getting latest release info...")
    with urllib.request.urlopen("https://api.github.com/repos/tailwindlabs/heroicons/releases/latest") as url:
        data = json.load(url)
        version = data["tag_name"]
        download_url = data["tarball_url"]

    # download tarball
    print(f"Downloading {version}...")
    with urllib.request.urlopen(download_url) as remote_file:
        value, params = cgi.parse_header(
            remote_file.info()['Content-Disposition'])
        tar_filename = params["filename"]

    # update heroicons version link in readme
    update_readme_heroicons_version(version)

    # create tmp directory
    Path("tmp").mkdir(exist_ok=True)
    tar_filename = f"tmp/{tar_filename}"
    urllib.request.urlretrieve(download_url, tar_filename)
    print(f"Downloaded latest release to {tar_filename}")

    # extract tar
    print("Extracting tar file...")
    tar = tarfile.open(tar_filename)
    tar.extractall(members=optimized_files(
        tar, os.path.commonprefix(tar.getnames())), path="tmp")
    tar.close()
    os.remove(tar_filename)

    # loop through files
    print("Looping through svg files and creating razor components...")
    create_blazor_component_files("Mini", "tmp/**/20/solid/*.svg")
    create_blazor_component_files("Solid", "tmp/**/24/solid/*.svg")
    create_blazor_component_files("Outline", "tmp/**/24/outline/*.svg")

    # clean up
    print("Cleaning up temp files...")
    shutil.rmtree("tmp")


def create_blazor_component_files(icon_type, glob):
    print(f"Creating {icon_type} razor components...")

    # remove all existing components
    componentDir = f"src/Blazor.Heroicons/{icon_type}"
    shutil.rmtree(componentDir)
    Path(componentDir).mkdir(exist_ok=True)

    # loop through svg files
    file_list = [f for f in iglob(glob, recursive=True) if os.path.isfile(f)]
    for file in file_list:
        content = ""
        # convert svg content to blazor component
        with open(file) as svg:
            content = "@inherits HeroiconBase\n"
            content = content + svg.read()
            content = content.replace("aria-hidden=\"true\">",
                                      "aria-hidden=\"true\" @attributes=\"AdditionalAttributes\">")

        # write file
        with open(f"{componentDir}/{to_pascal_case(Path(file).stem)}Icon.razor", 'w') as blazor_component:
            blazor_component.write(content)

    print(f"Created {len(file_list)} {icon_type} razor components")


def optimized_files(members, root):
    for tarinfo in members:
        if tarinfo.name.startswith(f"{root}/optimized"):
            yield tarinfo


def to_pascal_case(text):
    s = text.replace("-", " ").replace("_", " ")
    return "".join(s.title().split())


def update_readme_heroicons_version(version_tag):
    print("Updating heroicons version in README...")
    version_badge = ("[![Heroicons version]"
                     f"(https://img.shields.io/badge/heroicons-{version_tag}-informational?style=flat-square)]"
                     f"(https://github.com/tailwindlabs/heroicons/releases/tag/{version_tag})"
                     "\n")

    # read readme file
    with open("README.md", "r") as readme_file:
        lines = readme_file.readlines()
        for i in range(len(lines)):
            if lines[i].startswith("[![Heroicons version]"):
                lines[i] = version_badge

    with open("README.md", "w") as readme_file:
        readme_file.writelines(lines)
    with open(".heroicons-version", "w") as version_file:
        version_file.write(version_tag)


if __name__ == "__main__":
    main()
