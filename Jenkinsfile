#!/usr/bin/env groovy

pipeline {
    agent { label 'jenkins-slave-windows'}
    options { 
        timestamps() 
    }

    stages {
        stage('Publish') {
            steps {
                powershell '''
                    bundle install
                    bundle exec rake pack
                '''
            }
        }

        stage('tests') {
            steps {
                powershell 'src\\packages\\NUnit.ConsoleRunner.3.8.0\\tools\\nunit3-console.exe Tests\\bin\\Release\\Tests.dll --result="nunit-result.xml;format=nunit2"'
            }
            post {
                always {
                    nunit testResultsPattern: 'nunit-result.xml'
                }
            }
        }
    }

    post {
        cleanup {
            step([$class: 'WsCleanup'])
        }
    }
}
