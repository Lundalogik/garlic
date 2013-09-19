
require 'albacore'

namespace :garlic do
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
end
