#tool nuget:?package=NUnit.ConsoleRunner&version=3.4.0
#tool "nuget:?package=xunit.runner.console"


var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");


// Define directories.
var buildDir = Directory("./YahooFinance.NET/bin") + Directory(configuration);
var buildTestDir = Directory("./YahooFinance.NET.Tests/bin") + Directory(configuration);


Task("Clean")
    .Does(() =>
{
    CleanDirectory(buildDir);
    CleanDirectory(buildTestDir);
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore("./YahooFinance.NET.sln");
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
      MSBuild("./YahooFinance.NET.sln", settings =>
        settings.SetConfiguration(configuration));
});

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
{
    XUnit2(buildTestDir.ToString() + "/*.Tests.dll");
});

Task("CleanupOldNugetPackages")
    .Does(() =>
{
    var oldPackages = GetFiles("YahooFinance.NET*.nupkg");
    foreach(var file in oldPackages)
    {
        DeleteFile(file);
    }
});

Task("CreateNugetPackage")
    .IsDependentOn("Test")
    .IsDependentOn("CleanupOldNugetPackages")
    .Does(() =>
{
    var projectFile = new FilePath(@"YahooFinance.NET\YahooFinance.NET.csproj"); 

    var nuGetPackSettings = new NuGetPackSettings {
        Symbols = false,
        Properties = new Dictionary<string, string>
        {
            { "Configuration", configuration },
        }
    };

    NuGetPack(projectFile, nuGetPackSettings); 
});

Task("Deploy")
    .IsDependentOn("CreateNugetPackage")
    .Does(() =>
{
    var packages = GetFiles("YahooFinance.NET*.nupkg");

    Information("Deploying the following files...");
    foreach(var file in packages)
    {
        Information("File: {0}", file);
    }

    //This will fail unless the NuGet API Key is set
    //nuget.exe setApiKey <API-Key> -Source https://www.nuget.org/api/v2/package
    NuGetPush(packages, new NuGetPushSettings {
        Source = "https://www.nuget.org/api/v2/package",
    });
});

Task("Default")
    .IsDependentOn("Test");


RunTarget(target);
