# Langfoodi API
[![.NET](https://github.com/Andrei-Constantin-Programmer/LangfoodiAPI/actions/workflows/dotnet.yml/badge.svg)](https://github.com/Andrei-Constantin-Programmer/LangfoodiAPI/actions/workflows/dotnet.yml)

The robust API for the Langfoodi recipe-sharing social media platform.

## Table of Contents
- [Introduction](#introduction)
- [Features](#features)
- [Team](#team)
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

## Team  
[Andrei Constantin (myself)](https://github.com/Andrei-Constantin-Programmer)  
[Milovan Gveric](https://github.com/Unknown807)  
[Filip Fois](https://github.com/filip-edu)  
Nathan Gipson

## License
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
