# FixRussian letter C Analyzer

This is an analyzer and codefixer for Russian letter C in English word and vice versa.

An appropriate nuget can be found [here](https://www.nuget.org/packages/FixRussianCAnalyzer/)

## Enable this analyzer solution-wide

Insert to your `Directory.Build.props` the following:

```
  <PropertyGroup>    
    <WarningsAsErrors>RussianC;EnglishC</WarningsAsErrors>
  </PropertyGroup>

...

  <!-- анализатор некорректного использования букв C/С -->
  <ItemGroup Condition="'$(MSBuildProjectExtension)'=='.csproj'">
    <PackageReference Include="FixRussianCAnalyzer" Version="0.2.0">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
  </ItemGroup>

```

If you need to disable this analyzer in concrete file\lines you may add `#pragma warning disable RussianC` or `#pragma warning disable EnglishC` to the appropriate place.

