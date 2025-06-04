Pipeline Processor
==================


Content
=======
   
    * Summary of the solution
    * How to run the app
    * Instructions for setting up email notifications
    * How this could be extended

     
Summary of the solution
=======================

This is a C# .NET console application project to read notices from various gas pipeline websites and send notifications. Based on the notices, it identifies trading signals and sends out emails. 
The solution leverages C#, .NET 8, Docker, HTMLAgilityPack, MailKit etc, to provide a robust and scalable framework for scraping HTML notices, processing data, and sending emails.


How to run the App
==================

Follow these steps to set up and run the application locally:

Prerequisites
    
1. Install Visual Studio 2022 or later with .NET 8 SDK.
2. Ensure you have Docker Desktop installed if you want to run the application in a container.


Clone repository:

    git clone https://github.com/jhaakri/InCommoditiesTechChallenge.git


Build and publish the application:

1. Open the solution in Visual Studio.
2. Restore NuGet packages if prompted.
3. Build the solution (Ctrl + Shift + B).
4. Publish the application locally


To create a Docker image, you can use the provided Dockerfile. Ensure you have Docker installed and running on your machine.
    
1. Navigate to the project directory in your terminal.
2. Build the Docker image with the following command (It can be done from within the Visual Studio as well):
    
    docker build -t pipeline-processor .


Run the application:

1. You can run the application directly from Visual Studio by pressing F5 or Ctrl + F5.
2. If you built a Docker image, run the image from Docker Desktop


Instructions for setting up email notifications
===============================================
Setup an application account within the email account to use and generate an app-specific password. This is required to allow the application to send emails without using your main account password.
Set the correct values for the following in the appsettings.json file:

    "SmtpSettings": {
        "Server": "smtp.gmail.com",
        "Port": "587",
        "Email": "myaccount@gmail.com",
        "Password": "app password",
        "UseSSL": true,
        "EmailRetryTimeout": 500,
        "EmailRetryCount": 3
      }  

    "NotificationEmailSettings": {
        "To": "receiver@gmail.com",
        "Cc": "",
        "Bcc": "",
        "Subject": "Pipeline Notification Alert"
      }  


How this could be extended
==========================    

This solution provides the structure for a basic project setup to process one pipeline and send email notification. There are many things that can be done to enhance it and make it ready for production. Here are some of the things that should be enhanced:

1. Add more pipelines (e.g., different gas pipelines). It consists of adding configuration for the pipeline in appsettings.json file simply by duplicating the config from one of the existing pipeline and updating the configuration values. In addition, a scraper class needs to be added to scrape the Urls in the appsettings.
2. More relavent content like segment and volume should be extracted from the notices and used to generate more meaningful notifications.
3. Remove credential from the appsettings and store in a secure vault
4. Docker settings used is basic and can be enhanced to customize it better
5. Add more unit and integration tests to make sure the code coverage is at an acceptable level
6. Add additional notification channels (e.g., Slack, Microsoft Teams, SMS)
7. The business rules applied in this solution might not be complete, and should be enhanced to cover more scenarios
8. Setup a job/task to run the app automatically
9. Implement database integration for persistent storage of pipeline results and notifications
10. Add more logging mechanisms besides just the console output
11. Add a web dashboard using to monitor pipeline status and view logs.
12. Enhance the application to allow running specified pipeline tasks
13. SabineScraper class is an example of how to implement a scraper for a pipeline. It can be used as a template to create more scrapers for other pipelines. It is just a template and does not actually get the correct data for the pipeline. the xpath queries need to be adjusted to get this to work.
    
