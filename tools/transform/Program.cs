// See https://aka.ms/new-console-template for more information
using Scriban;

string simpleIconPath = args[0];
string outputPath = args[1];

Console.WriteLine($"input:{simpleIconPath} output:{outputPath}");

if (!Directory.Exists(simpleIconPath)) throw new Exception($"{simpleIconPath} does not exist");
var fullFileNames = Directory.EnumerateFiles(simpleIconPath);

foreach (string? fullFileName in fullFileNames)
{
    string iconName = Path.GetFileNameWithoutExtension(fullFileName);
    string iconExtension = Path.GetExtension(fullFileName);
    if (iconExtension != ".svg") return;

    char firstChar = iconName[0];
    bool firstCharIsValid = char.IsLetter(firstChar) || firstChar == '_';
    var remaingIconName = firstCharIsValid ? iconName.Remove(0, 1): iconName;

    firstChar = firstCharIsValid ? firstChar : '_';
    firstChar = char.ToUpper(firstChar);
    string suffix = "Icon";
    string componentName = componentName = $"{firstChar}{remaingIconName}{suffix}";
    string componentFullFileName = $"{outputPath}/{componentName}.razor";

    string? fileContent = File.ReadAllText(fullFileName);
    string content = Transform(fileContent, componentName);
    File.WriteAllText(componentFullFileName, content);
}

string Transform(string fileContent, string componentName)
{
    string search = "<svg role=\"img\" viewBox=\"0 0 24 24\" xmlns=\"http://www.w3.org/2000/svg\">";
    string replacement = "<svg role=\"img\" viewBox=\"0 0 24 24\" xmlns=\"http://www.w3.org/2000/svg\" @attributes=Attributes>";
    var svg = fileContent.Replace(search, replacement);

    var templateContent = File.ReadAllText("template.scriban");
    var template = Template.Parse(templateContent);
    var componentContent = template.Render(new { svg = svg });
    return componentContent;
}