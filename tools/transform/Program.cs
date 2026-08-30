#region Purpose
// Converts simple-icons SVG files into Blazor icon components
#endregion
#region Design
// Template path is resolved from the binary dir / project dir, not CWD.
// CI update-icons runs this tool with working directory = repo root.
// Non-svg files in the input dir are skipped; they must not abort the loop.
#endregion

using Scriban;

string simpleIconPath = args[0];
string outputPath = args[1];

if (!Directory.Exists(simpleIconPath)) throw new Exception($"{simpleIconPath} does not exist");

string templatePath = ResolveTemplatePath();
string templateContent = File.ReadAllText(templatePath);
Template template = Template.Parse(templateContent);

foreach (string fullFileName in Directory.EnumerateFiles(simpleIconPath))
{
  string iconName = Path.GetFileNameWithoutExtension(fullFileName);
  string iconExtension = Path.GetExtension(fullFileName);
  if (iconExtension != ".svg") continue;

  char firstChar = iconName[0];
  bool firstCharIsValid = char.IsLetter(firstChar) || firstChar == '_';
  string remainingIconName = firstCharIsValid ? iconName.Remove(0, 1) : iconName;

  firstChar = firstCharIsValid ? firstChar : '_';
  firstChar = char.ToUpper(firstChar);
  string suffix = "Icon";
  string componentName = $"{firstChar}{remainingIconName}{suffix}";
  string componentFullFileName = $"{outputPath}/{componentName}.razor";

  string fileContent = File.ReadAllText(fullFileName);
  string content = Transform(fileContent, template);
  File.WriteAllText(componentFullFileName, content);
}

static string ResolveTemplatePath()
{
  string[] candidates =
  [
    Path.Combine(AppContext.BaseDirectory, "template.scriban"),
    Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "template.scriban")),
    Path.Combine(Environment.CurrentDirectory, "template.scriban"),
    Path.Combine(Environment.CurrentDirectory, "tools", "transform", "template.scriban"),
  ];

  foreach (string candidate in candidates)
  {
    if (File.Exists(candidate)) return candidate;
  }

  throw new FileNotFoundException(
    "Could not find template.scriban. Expected it next to the transform binary (CopyToOutputDirectory) or under tools/transform/.",
    "template.scriban");
}

static string Transform(string fileContent, Template template)
{
  string search = "<svg role=\"img\" viewBox=\"0 0 24 24\" xmlns=\"http://www.w3.org/2000/svg\">";
  string replacement = "<svg role=\"img\" viewBox=\"0 0 24 24\" xmlns=\"http://www.w3.org/2000/svg\" @attributes=Attributes>";
  string svg = fileContent.Replace(search, replacement);

  return template.Render(new { svg = svg });
}
