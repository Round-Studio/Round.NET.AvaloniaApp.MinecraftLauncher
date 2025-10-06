@echo off

taskkill /f /im RMCL.exe
dotnet publish ./src/PluginTools/PluginTools.csproj -c Release -o ./build/PluginTools
powershell ./build/PluginTools/PluginTools.exe -b -config ./buildConfig/Plugin.BedrockBoot.json
powershell copy "build/Plugin.BedrockBoot/publish/pack.rplck" "%APPDATA%\RoundStudio\RMCL4\RMCL.Plugin"