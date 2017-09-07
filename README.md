# Energy Management Mobile App

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes. See deployment for notes on how to deploy the project on a live system.

### Prerequisites

1. Energy Management Web App and REST Server deployement 
2. REST API Server URL for configuring mobile apps.
3. Supports Android OS 4+.
4. Supports IOS 8+.
5. Allows Notification access for apps.

```
    Sample Rest API Url: https://emtest.azurewebsites.net
```

### Introduction - Energy Management App

Energy Management Applications are available for Android and IOS devices. This app have features as follows
1. <b>Login:</b> User can Sign Up/ Sign In using this option. Azure Active directory B2C implementations required for login configuration.
2. <b>Admin Dashboard:</b> Showcase Energy Consumptions details for Admin User.
3. <b>Insights:</b> Provides energy consumption recommendations for Admin User.
4. <b>Alerts:</b> Provides alerts for Energy consumptions insights, new devices and feedback.
5. <b>Feedback:</b> Non admin user can only share Feedback. 


### Installing

Download the app from shared url and copy it in your device.
    
1. Go to local path where app is stored.
2. click on app to start installation.
3. Once installation is complete, " Energy Management " app will appear on your menu.
4. Click on Energy Management to start app. When app is started for first time, it will ask for "REST API Url" on config screen.
5. Enter REST API URL and submit.
6. It will connect to REST Server and now app is ready to use.



## Running the tests

Explain how to run the automated tests for this system

### Break down into end to end tests

Explain what these tests test and why

```
Give an example
```

### And coding style tests

Explain what these tests test and why

```
Give an example
```

## Deployment

Add additional notes about how to deploy this on a live system

## Built With

* [Dropwizard](http://www.dropwizard.io/1.0.2/docs/) - The web framework used
* [Maven](https://maven.apache.org/) - Dependency Management
* [ROME](https://rometools.github.io/rome/) - Used to generate RSS Feeds

## Contributing

Please read [CONTRIBUTING.md] for details on our code of conduct, and the process for submitting pull requests to us.

## Versioning

We use [SemVer](http://semver.org/) for versioning. For the versions available, see the [tags on this repository](https://github.com/your/project/tags). 

## License

Currently keep this open