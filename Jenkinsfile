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

        stage('Create Nuget package') {
            when {
                branch 'master'
            }
            steps {
                script {
                    if (env.BRANCH_NAME == 'master') {
                        powershell '''
                            bundle exec rake pack
                        '''
                        } 
                    } else {
                        powershell '''
                            bundle exec rake pack['-rc.$ENV:BUILD_ID']
                        '''
                        } 
                    }
                }
            }
        }

        stage('Publish Nuget package') {
            when {
                branch 'master'
            }
            steps {
                powershell '''
                    bundle exec rake publish
                '''
            }
        }
    }
}
