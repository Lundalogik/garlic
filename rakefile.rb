
require 'albacore'

filepath = File.dirname(__FILE__)
desc "Build the solution (debug)"
msbuild :build do |msb|
  msb.properties :configuration => :Debug , :Platform => 'Any CPU'
  msb.verbosity = 'minimal'
  msb.targets :Rebuild
  msb.solution = File.join(filepath,'Garlic.sln')
end


msbuild :cleanmsbuild do |msb|
  msb.properties :configuration => :Debug 
  msb.verbosity = 'minimal'
  msb.targets :Clean
  msb.solution = File.join(filepath,'Garlic.sln')
end

targets = ['Garlic', 'Tests']

desc "Clean the solution"
task :clean => [:cleanmsbuild] do
  targets.each do |target|
    rm_rf(File.join(filepath,target,'bin','debug'))
  end
end

task :tests => [:unittests]

desc "Run c# unit tests"
nunit :unittests => [:build] do |nunit|
  nunit.command = Dir.glob(File.join(filepath,'packages','NUnit.*','tools','nunit-console.exe')).first
  nunit.assemblies File.join(filepath,'Tests','bin','debug','Tests.dll')
end

def nuget_folder
  return 'nuget'
end

def nuget_lib40
  return File.join(nuget_folder, 'lib', '40')
end

task :clean_nuget_folder do
  rm_rf(nuget_folder)
end

task :core_copy_to_nuspec => [:clean_nuget_folder, :build, :nuget_folder] do
  cp Dir.glob("./Garlic/bin/Debug/Garlic.dll"), nuget_lib40
end

task :nuget_folder do
  mkdir_p(nuget_folder)
  mkdir_p(nuget_lib40)
end

desc "create the nuspec file"
nuspec :create_spec => [:core_copy_to_nuspec, :nuget_folder] do |nuspec|
  GARLIC_VERSION = '1.0.0.3'

  nuspec.id = "GarlicMigrations"
  nuspec.version = GARLIC_VERSION
  nuspec.authors = "Peter Wilhelmsson"
  nuspec.owners = "Peter Wilhelmsson"
  nuspec.description = "Migrate databases using .net."
  nuspec.title = "Garlic Migrations"
  nuspec.language = "en-US"
  nuspec.projectUrl = "https://github.com/2hdddg"
  nuspec.working_directory = "nuget"
  nuspec.output_file = "GarlicMigrations.nuspec"
  nuspec.dependency('Npgsql','2.2.0')
end

desc "package nuget package"
task :pack => [:create_spec] do
  cd nuget_folder do
    sh "..\\.nuget\\NuGet.exe pack GarlicMigrations.nuspec"
  end
end

