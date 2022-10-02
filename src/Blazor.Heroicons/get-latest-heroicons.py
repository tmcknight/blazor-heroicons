from glob import iglob
import os
import urllib.request
import json
from pathlib import Path
import cgi
import shutil
import tarfile


def main():
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
    print("Cleaning up tmp files...")
    shutil.rmtree("tmp")


def create_blazor_component_files(icon_type, glob):
    print(f"Creating {icon_type} razor components...")

    # remove all existing components
    shutil.rmtree(icon_type)
    Path(icon_type).mkdir(exist_ok=True)

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
        with open(f"{icon_type}/{to_title_case(Path(file).stem)}Icon.razor", 'w') as blazor_component:
            blazor_component.write(content)

    print(f"Created {len(file_list)} {icon_type} razor components")


def optimized_files(members, root):
    for tarinfo in members:
        if tarinfo.name.startswith(f"{root}/optimized"):
            yield tarinfo


def to_title_case(text):
    s = text.replace("-", " ").replace("_", " ")
    s = s.split()
    if len(text) == 0:
        return text
    return s[0].capitalize() + ''.join(i.capitalize() for i in s[1:])


if __name__ == "__main__":
    main()
