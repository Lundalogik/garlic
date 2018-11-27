# coding: iso-8859-1

task :default => [ :build, :tests ]

solutionpath = File.join(File.dirname(__FILE__),'Garlic.sln')
testpath = File.join(File.dirname(__FILE__),'\Tests\Tests.csproj')
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
