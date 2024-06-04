<div align="center">
  <picture>
    <source media="(prefers-color-scheme: dark)" srcset="https://github.com/Andrei-Constantin-Programmer/LangfoodiAPI/assets/55529876/a85bc7c6-85f1-47f2-9492-6c9b8fcac7b6">
    <source media="(prefers-color-scheme: light)" srcset="https://github.com/Andrei-Constantin-Programmer/LangfoodiAPI/assets/55529876/f06925f0-f036-43fc-90c9-b8a9a3bbe430">
    <img alt="Langfoodi Logo." src="https://github.com/Andrei-Constantin-Programmer/LangfoodiAPI/assets/55529876/f06925f0-f036-43fc-90c9-b8a9a3bbe430" height="40%" width="40%">
  </picture>
</div>

# Langfoodi API
[![.NET](https://github.com/Andrei-Constantin-Programmer/LangfoodiAPI/actions/workflows/dotnet.yml/badge.svg)](https://github.com/Andrei-Constantin-Programmer/LangfoodiAPI/actions/workflows/dotnet.yml)

The robust API for the Langfoodi recipe-sharing social media platform.


## Table of Contents
- [Introduction](#introduction)
- [Features](#features)
- [Installation Guide](#installation)
  - [Prerequisites](#prerequisites)
  - [With Visual Studio](#with-vs)
  - [With command line](#with-cmd) 
- [Team](#team)
- [Acknowledgements](#acknowledgements)
- [License](#license)



## Introduction  
Langfoodi is a recipe-sharing social media platform that combines recipe management with messaging functionalities.  

Langfoodi has been my group project dissertation during my Bachelor's degree at the University of Kent, on which I worked together with my [team](#team).  

This repository contains the API that services the mobile app for Langfoodi, and serves as the validator, coordinator, and interface with storage solutions. 



## Features
The API provides endpoints that allow the mobile app to:
- Register/authenticate users
- Create/modify/delete/view recipes
- Create connections between users
- Create groups of users
- Send/update/delete messages
- Interact with Cloudinary

Additionally, the API provides the SignalR hub `MessagingHub` to allow for live messaging between users. Clients subscribed to it will receive signals when messages are received, read, deleted etc.

The data is stored in MongoDB, with sensitive data (user PII, passwords, message contents etc.) encrypted. Images are stored in Cloudinary.



## Installation
### Prerequisites
Before you start, ensure you have the following set up on either your machine or the cloud:
- **.NET 7 SDK** - Download and install from the [.NET 7 SDK download page](https://dotnet.microsoft.com/download/dotnet/7.0)

- **MongoDB Cluster** - Create your own Mongo cluster (see the [MongoDB tutorial](https://www.mongodb.com/docs/atlas/tutorial/create-new-cluster/))

- **Cloudinary Environment** - To use images, you will need a Cloudinary environment (see the [Cloudinary tutorial](https://cloudinary.com/documentation/how_to_integrate_cloudinary))

- (Optional) **IDE** (recommended: [Visual Studio 2022](https://visualstudio.microsoft.com/downloads/))

  -  If using Visual Studio, ensure you have the `.NET desktop development` and `ASP.NET and web development` workloads installed

- (Optional) **DataDog Organisation** (follow the [DataDog setup wizard](https://www.datadoghq.com/))



<a name="with-vs"></a>
### Installation - Using Visual Studio 2022
1. **Clone the Repository** either:

    - Directly from GitHub: from the [repository page](https://github.com/Andrei-Constantin-Programmer/LangfoodiAPI) > Code > Open with Visual Studio

    - or From Visual Studio itself: Git > Clone Repository, using "https://github.com/Andrei-Constantin-Programmer/LangfoodiAPI.git"

2. **Open the Solution** - File > Open > Project/Solution (or `CTRL+SHIFT+O`), and select the .sln file in the root of the repository
<a name="app-settings"></a>
3. **Add appsettings** - You need to add your own API keys and secrets in RecipeSocialMediaAPI.Presentation > appsettings.json  
    - **Cloudinary** [cloud name, API key, and API secret](https://cloudinary.com/documentation/cloudinary_credentials_tutorial)  

    - **MongoDB** [connection string](https://www.mongodb.com/resources/products/fundamentals/mongodb-connection-string#:~:text=How%20to%20get%20your%20MongoDB,connection%20string%20for%20your%20cluster.) and [cluster name](https://www.mongodb.com/resources/products/fundamentals/clusters)

    - (Optional) **DataDog** [API key](https://docs.datadoghq.com/account_management/api-app-keys/)  

    - (Optional) Custom JWT key and encryption key  

4. **Run API** - Select `RecipeSocialMediaAPI.Presentation` as your startup project and run the API (`F5` or the `Start` button in the toolbar)  
   This should also open **Swagger**, where you can view the endpoints and their documentation.



<a name="with-cmd"></a>
### Installation - Using command line
1. **Clone the Repository**:

   ```bash
   git clone https://github.com/Andrei-Constantin-Programmer/LangfoodiAPI.git
   ```
2. **Navigate to the Startup Project**

   ```bash
   cd LangfoodiAPI/RecipeSocialMediaAPI.Presentation
   ```
3. **Restore Dependencies**

   ```bash
   dotnet restore
   ```
4. **Update appsettings.json** using a text/code editor of your choice (see the [steps above](#app-settings))

5. **Build Project**

   ```bash
   dotnet build
   ```
6. **Run Project**

   ```bash
   dotnet run
   ```
7. (Optional) **Navigate to Swagger** to see endpoint documentation  
  The endpoints should be visible under https://localhost:xxxx/swagger/index.html (check console logs for `Now listening on:` to get port)



## Team  
[Andrei Constantin (myself)](https://github.com/Andrei-Constantin-Programmer)  
[Milovan Gveric](https://github.com/Unknown807)  
[Filip Fois](https://github.com/filip-edu)  
Nathan Gipson


## Acknowledgements
I would like to thank George Langroudi for supervising our dissertation.

I would also like to thank the University of Kent for the opportunity to work on this project.


## License
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
