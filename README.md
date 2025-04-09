# WorkHive Backend API

## Overview

This project is written in **C#** and utilizes the **.NET framework**. It includes various components such as APIs, data repositories, and service layers to support the backend functionality of the **WorkHive** application.

## Main Features

- Provides backend APIs for managing core functionalities of the **WorkHive** platform, including:
  - Booking management
  - User management
  - Feedback system
  - Other related business features
- Implements a modular architecture following the **CQRS (Command Query Responsibility Segregation)** pattern.
- Applies clean architecture principles with clear separation between application, domain, and infrastructure layers.
- Includes **unit tests** to ensure reliability and correctness of the API implementations.
- Supports containerization using **Docker**.

## Technology Stack

- **C#**
- **.NET Framework**
- **CQRS (Command Query Responsibility Segregation)**
- **Docker**

## Getting Started

To run the application locally using Docker:

```bash
docker build -t workhive-api .
docker run -p 5000:80 workhive-api
