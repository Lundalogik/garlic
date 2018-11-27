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
            steps {
                script {
                    if (env.BRANCH_NAME == 'master') {
                        powershell '''
                            bundle exec rake pack
                        '''
                    } else {
                        powershell '''
                            $env:BUILD_PREFIX = -join("rc.", $ENV:JOB_BASE_NAME.ToLower(),"_",$ENV:BUILD_ID)
                            bundle exec rake pack[$ENV:BUILD_PREFIX]
                        '''
                    }
                }
            }
        }

        stage('Publish Nuget package') {
            steps {
                script {
                    if (env.BRANCH_NAME == 'master') {
                        powershell '''
                            bundle exec rake publish
                        '''
                    } else {
                        powershell '''
                            bundle exec rake publish['rc.${ENV:BUILD_ID}    ']
                        '''
                    }
                }
            }
        }
    }
}
