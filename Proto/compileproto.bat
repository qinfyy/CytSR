@echo off

rmdir /S /Q CSharp
mkdir CSharp
dotnet tool run protogen --proto_path="%~dp0Proto\\" "*.proto" --csharp_out="CSharp"
dotnet tool run protogen --proto_path="%~dp0ServerProto\\" "*.proto" --csharp_out="CSharp"
pause
