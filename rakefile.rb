# coding: iso-8859-1

task :default => [ :build, :tests ]

filepath = File.join(File.dirname(__FILE__),'Garlic.sln')
desc "Build the solution (debug)"
task :build do
  system("dotnet build --configuration Debug #{filepath}")
end

task :build_release do
  system("dotnet build --configuration Release #{filepath}")
end

desc "Clean the solution"
task :clean do
  system("dotnet clean #{filepath}")
end

task :tests => ['test:unit']

desc "Run c# unit tests"
namespace :test do
  task :unit do
    system("dotnet test #{filepath} --logger:\"nunit;LogFilePath=../test-result.xml\"")
  end
end
