## MSBuild build routine

### Prerequisites
1. Important to run next command once from IDE Terminal _(Rider:(Ctrl+Alt+1), Visual Studio:(Ctrl+` or View>Terminal))_

_Windows_(For linux and Mac look [here](https://github.com/microsoft/artifacts-credprovider#setup))
```powershell
iex "& { $(irm https://aka.ms/install-artifacts-credprovider.ps1) } -AddNetfx"
```
It will install Azure Artifacts [CredProvider](https://github.com/microsoft/artifacts-credprovider) to your environment and you can use `dotnet restore` from Terminal for restore packages from Stenn private nuget repository
2. After it run next command and follow instructions from console to authenticate
```powershell
dotnet restore --interactive
```
_NOTE: In console output you will find authentication CODE and link to the Microsoft website with authentication. If you don't have authentication token yet_

_All these steps need for successful restoring packages from private Nuget repository._

### Structure

#### All main build parameters controlled by files in .msbuild folder.
* `appservices.contracts.nuget.packages.versions.targets` is [nuget versions single file](https://www.notion.so/Nuget-targets-f3f3e6a4f85c4a5cae4bd3ed83c4f196) for app services contracts used by projects.
* `build.props` contains main parameters, like TargetFrameworks and etc.
* `nuget.packages.versions.targets` is [nuget versions single file](https://www.notion.so/Nuget-targets-f3f3e6a4f85c4a5cae4bd3ed83c4f196) for 3rd party packages used current app service only.
* `seedwork.props` contains current using version of Seedwork

#### .msbuild\utils
Contains routines of build process
* `Directory.Build.props` entry point for imports of `.props` files _(Located in solution folder)_.
* `Directory.Build.targets` entry point for imports of `.targets` files _(Located in solution folder)_.

#### .msbuild\seedwork.restore
Special routine for restoring Seedwork shared build configuration that used before main nuget package restoring
Details you can find in README.md inside the folder.

__IMPORTANT:__ Do not change any files in the folder. It shared though all codebase. Contact Libs team in any weird circumstances

#### .msbuild\seedwork
Storage for current Seedwork version imports. Details you can find in README.md inside the folder.

__IMPORTANT:__ Do not change any files in this folder. This folder will updated automatically during restore seedwork.msbuild.packages. seedwork version controlled by property SeedworkVersion in build.props

#### .msbuild\local.profile.props (and targets)
Local settings for project building. May include local seedwork attachment settings.  
For details, see [here](https://www.notion.so/Local-profile-props-targets-f4e6f2c1327f41a7a117f6ba38c27f70)

### Update Seedwork version

For update Seedwork version:
1. Change version in `seedwork.props`
2. Run `dotnet restore -f --no-cache --interactive` in a Terminal window
3. If you got next error `Package 'Seedwork.MSBuild.Packages.$(you changed version)' with Seedwork packages' versions restored. Run build again` run step 2 again.
4. Reload all projects for invalidate IDE intellisense caches
    * Rider:Right click on the solution in Explorer and select `Reload All Projects`.

### Change branch
After changing a branch
1. Clean and restore nuget packages
    * Rider(_Terminal_): `dotnet clean ; dotnet restore --interactive`
    * Visual Studio(_Package Manager Console_): `dotnet clean ; dotnet restore` or just use Cleanup on solution
2. If you have any error during restore try next(_It's heavier because it skips your local Nuget packages' cache_):
    * Rider(_Terminal_): `dotnet clean ; dotnet restore --interactive --no-cache`
    * Visual Studio(_Package Manager Console_): `dotnet clean ; dotnet restore --no-cache`
3. Reload all projects for invalidate IDE intellisense caches
    * Rider:Right click on the solution in Explorer and select `Reload All Projects`.