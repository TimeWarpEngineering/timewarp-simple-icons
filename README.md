[![Dotnet](https://img.shields.io/badge/dotnet-6.0-blue)](https://dotnet.microsoft.com)
[![Stars](https://img.shields.io/github/stars/TimeWarpEngineering/timewarp-simple-icons?logo=github)](https://github.com/TimeWarpEngineering/timewarp-simple-icons)
[![Discord](https://img.shields.io/discord/715274085940199487?logo=discord)](https://discord.gg/7F4bS2T)
[![nuget](https://img.shields.io/nuget/v/timewarp-simple-icons?logo=nuget)](https://www.nuget.org/packages/timewarp-simple-icons/)
[![nuget](https://img.shields.io/nuget/dt/timewarp-simple-icons?logo=nuget)](https://www.nuget.org/packages/timewarp-simple-icons/)
[![Issues Open](https://img.shields.io/github/issues/TimeWarpEngineering/timewarp-simple-icons?logo=github)](https://github.com/TimeWarpEngineering/timewarp-simple-icons/issues)
[![Forks](https://img.shields.io/github/forks/TimeWarpEngineering/timewarp-simple-icons)](https://github.com/TimeWarpEngineering/timewarp-simple-icons)
[![License](https://img.shields.io/github/license/TimeWarpEngineering/timewarp-simple-icons?logo=github)](https://unlicense.org)
[![Twitter](https://img.shields.io/twitter/url?style=social&url=https%3A%2F%2Fgithub.com%2FTimeWarpEngineering%2Ftimewarp-simple-icons)](https://twitter.com/intent/tweet?url=https://github.com/TimeWarpEngineering/timewarp-simple-icons)

[![Twitter](https://img.shields.io/twitter/follow/StevenTCramer.svg)](https://twitter.com/intent/follow?screen_name=StevenTCramer)
[![Twitter](https://img.shields.io/twitter/follow/TheFreezeTeam1.svg)](https://twitter.com/intent/follow?screen_name=TheFreezeTeam1)

# timewarp-simple-icons

![TimeWarp Logo](assets/Logo.png)

All [simple-icons](https://github.com/simple-icons/simple-icons) wrapped as Blazor components.
See and search all at https://simpleicons.org/

## Give a Star! :star:

If you like or are using this project please give it a star. Thank you!

## Usage

```razor
<GithubIcon class="w-6 h-6"/>
<BlazorIcon class="w-6 h-6 fill-[#512BD4]" />
<TwitterIcon class="w-6 h-6 fill-blue-400" />
```

Outputs

![](assets/20220731140539.png)  

## Installation

You can see the latest NuGet packages from the official [TimeWarp NuGet page](https://www.nuget.org/profiles/TimeWarp.Enterprises).

* [timewarp-simple-icons](https://www.nuget.org/packages/timewarp-simple-icons/) [![nuget](https://img.shields.io/nuget/v/timewarp-simple-icons?logo=nuget)](https://www.nuget.org/packages/timewarp-simple-icons/)

```console
dotnet add package timewarp-simple-icons
```

## License

[![License](https://img.shields.io/github/license/TimeWarpEngineering/timewarp-simple-icons?logo=github)](https://unlicense.org)

## Contributing

Time is of the essence.  Before developing a Pull Request I recommend opening a [discussion](https://github.com/TimeWarpEngineering/timewarp-simple-icons/discussions).

Please feel free to make suggestions and help out with the [documentation](https://timewarpengineering.github.io/timewarp-simple-icons/).
Please refer to [Markdown](http://daringfireball.net/projects/markdown/) for how to write markdown files.

### Steps to publish NuGet package

* [ ] Clone the [simple-icons](https://github.com/simple-icons/simple-icons) repo.
* [ ] Set the PowerShell variable `$simple_icons` to the path where you cloned the simple-icons repo in the above step. (Add `$simple_icons = "<PATH where you stored the repp>"` to your profile)
* [ ] Ensure your copy of the simple-icons repo is up to date by running (`update.ps1`).
* [ ] Set the Version in `timewarp-simple-icons/source/timewarp-simple-icons/timewarp-simple-icons.csproj` to the same version that is in `simple-icons/package.json`.
* [ ] Transform the cloned [simple-icons](https://github.com/simple-icons/simple-icons) into razor files by running `transform.ps1`.
* [ ] Run the test app to make sure the icons render properly.
* [ ] Update `releases.md`.
* [ ] Commit and push the changes to GitHub.
* [ ] Set the PowerShell variable `$Nuget_Key` value.
* [ ] Publish to NuGet by running `publish.ps1`.
* [ ] Tweet to let people know.

## Contact

Sometimes the GitHub notifications get lost in the shuffle.  If you file an [issue](https://github.com/TimeWarpEngineering/timewarp-simple-icons/issues) and don't get a response in a timely manner feel free to contact us on our [Discord server](https://discord.gg/A55JARGKKP).

[![Discord](https://img.shields.io/discord/715274085940199487?logo=discord)](https://discord.gg/7F4bS2T)

## References

https://github.com/simple-icons/simple-icons

### Commands used

```PowerShell
dotnet new sln
dotnet new razorclasslib -n timewarp-simple-icons
dotnet sln add .\Source\timewarp-simple-icons\timewarp-simple-icons.csproj
dotnet new tool-manifest
dotnet tool install dotnet-cleanup
dotnet cleanup -y
```
