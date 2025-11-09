@echo off

md "%APPDATA%\RoundStudio\RMCL4\RMCL.Plugin"
dotnet publish ./src/PluginTools/PluginTools.csproj -c Release -o ./build/PluginTools
powershell ./build/PluginTools/PluginTools.exe -b -config ./buildConfig/Plugin.BedrockBoot.json
powershell ./build/PluginTools/PluginTools.exe -b -config ./buildConfig/Plugin.MusicPlayer.json
powershell copy "build/Plugin.BedrockBoot/publish/pack.rplck" "%APPDATA%\RoundStudio\RMCL4\RMCL.Plugin\BedrockBoot.rplck.disable"
# powershell copy "build/Plugin.MusicPlayer/publish/pack.rplck" "%APPDATA%\RoundStudio\RMCL4\RMCL.Plugin\MusicPlayer.rplck"