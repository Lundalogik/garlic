# coding: iso-8859-1

task :default => [ :build, :tests ]

solutionpath = File.join(File.dirname(__FILE__),'Garlic.sln')
testpath = File.join(File.dirname(__FILE__),'\Tests\Tests.csproj')
projpath = File.join(File.dirname(__FILE__),'\GarlicMigrations\GarlicMigrations.csproj')
nugetpath = File.join(File.dirname(__FILE__),'\GarlicMigrations\bin\release').gsub('/', '\\')

desc "Build the solution (debug)"
task :build do
  system("dotnet build --configuration Debug #{solutionpath}")
end

task :build_release do
  system("dotnet build --configuration Release #{solutionpath}")
end

desc "Clean the solution"
task :clean do
  system("dotnet clean #{solutionpath}")
end

task :tests => ['test:unit']

desc "Run c# unit tests"
namespace :test do
  task :unit do
    system("dotnet test #{testpath} --logger:\"nunit;LogFilePath=../nunit-result.xml\"")
  end
end

desc "Build NuGet package"
task :pack, [:suffix] do |t, args|
  suffix = args[:suffix] || ''
  if suffix == ''
    system("dotnet pack --configuration release #{projpath}")
  else 
    system("dotnet pack --configuration release --version-suffix #{suffix} #{projpath}")
  end
end

desc "Publish NuGet package"
task :publish, [:key, :suffix] do |t, args|
  apikey = args[:key] || ''
  suffix = args[:suffix] ? '-'+args[:suffix] : ''
  system("dotnet nuget push #{nugetpath}\\GarlicMigrations.*#{suffix}.nupkg -k #{apikey} -s https://api.nuget.org/v3/index.json")
end
