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
                powershell '''
                    bundle exec rake tests
                '''
            }
            post {
                always {
                    nunit testResultsPattern: 'nunit-result.xml'
                }
            }
        }

        stage('Upload Nuget') {
            steps {
                echo "Not done yet :-("
            }
        }
    }
}
