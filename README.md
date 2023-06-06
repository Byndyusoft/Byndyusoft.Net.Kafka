[![License](https://img.shields.io/badge/License-Apache--2.0-blue.svg)](https://opensource.org/licenses/Apache-2.0)

# .NET Nuget publishing template
This is a template repository with example github actions for .NET nuget packages creation and publishing

# How to use:
 - Remove example solution (ExampleSolution.sln, src and tests folders)
 - add icon reference to packaged projects ([example](src/ExampleProject/ExampleProject.csproj))
 - Change properties in Directory.Build.props file according to your needs (version, package tags, repository url)
 - fix **dotnet-version** in .github/workflows/\*.yml

# How to publish pre-release to nuget.org:

Mark *This is a pre-release* checkbox when you create a release. 

![image](https://user-images.githubusercontent.com/38452272/184600138-abc74f6e-3c7e-4c0a-ad51-426473f02917.png)

The package version will be *<proj_version>-tags-<tag_name>* where *proj_version* is retrieved from .csproj or Directory.Build.props file.

# Maintainers
[github.maintain@byndyusoft.com](mailto:github.maintain@byndyusoft.com)
