# coding: iso-8859-1

require 'albacore'

task :default => [ :build, :tests ]

filepath = File.dirname(__FILE__)
desc "Build the solution (debug)"
build :build => :restore do |msb|
  msb.prop :configuration, :Debug 
  msb.prop :Platform, 'Any CPU'
  msb.logging = 'minimal'
  msb.target = :Rebuild
  msb.sln = File.join(filepath,'Garlic.sln')
end

build :build_release => :restore do |msb|
  msb.prop :configuration, :Release 
  msb.prop :Platform, 'Any CPU'
  msb.logging = 'minimal'
  msb.target = :Rebuild
  msb.sln = File.join(filepath,'Garlic.sln')
end

nugets_restore :restore do |p|
  p.out       = 'packages'       # required
  p.exe       = '.nuget/NuGet.exe'    # required
  p.list_spec = '**/packages.config'
end

build :cleanmsbuild do |msb|
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

task :tests => ['test:unit']

desc "Run c# unit tests"
namespace :test do
  
  task :unit do
    nunit_command = Dir.glob(File.join(filepath,'packages','NUnit.*','tools','nunit-console.exe')).first
    nunit_assemblies = File.join(filepath,'Tests','bin','debug','Tests.dll')
    system "#{nunit_command} #{nunit_assemblies}", clr_command: true
  end
end

def nuget_folder
  return 'nuget'
end

def nuget_lib
  return File.join(nuget_folder, 'lib', '451')
end

task :clean_nuget_folder do
  rm_rf(nuget_folder)
end

task :nuget_folder do
  mkdir_p(nuget_folder)
  mkdir_p(nuget_lib)
end

desc "package nuget package"
nugets_pack :pack => [:build_release, :clean_nuget_folder, :nuget_folder] do |p|
  GARLIC_VERSION = '1.6.0.1'
  
  p.configuration = 'Release'
  p.files   = FileList['**/*.{csproj,fsproj,nuspec}'].exclude(/Tests/)
  p.out     = 'nuget'
  p.target  = 'net451'
  p.exe     = '.nuget/nuget.exe'
  # This line will leave the nuspec so you can inspect and verify it, take it out if you don't want the nuspec file to
  # stay around.
  p.leave_nuspec
  p.with_metadata do |m|
    m.id = "GarlicMigrations"
    m.version = GARLIC_VERSION
    m.authors = "Peter Wilhelmsson, Anders Pålsson, Petter Sandholdt"
    m.owners = "Lime Technologies AB"
    m.description = "Migrate databases using .net."
    m.title = "Garlic Migrations"
    m.language = "en-US"
    m.add_dependency 'Npgsql','3.2.7'
  end
end