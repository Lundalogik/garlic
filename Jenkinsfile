#!/usr/bin/env groovy

pipeline {
    agent { label 'jenkins-slave-windows'}
    options { 
        timestamps() 
    }

    stages {
        stage('Build') {
            steps {
                powershell '''
                    bundle install
                    bundle exec rake build_release
                '''
            }
        }

        stage('Tests') {
            steps {
                bat 'src\\packages\\NUnit.ConsoleRunner.3.8.0\\tools\\nunit3-console.exe src\\Go.Tests.Integration\\bin\\Release\\Go.Tests.Integration.dll --result="nunit-result.xml;format=nunit2"'
            }
            post {
                always {
                    nunit testResultsPattern: 'nunit-result.xml'
                }
            }
        }

        stage('Create Nuget') {
            steps {
                echo "Not done yet :-("
            }
        }
    }
}
