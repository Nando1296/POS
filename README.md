# POS (Point of Sale) System

A complete microservices-based Point of Sale application. This repository contains the source code for the distributed services, API gateways, and client applications.

## Architecture & Technologies

This system is built focusing on scalability, maintainability, and high performance using the following stack:

* Framework: .NET 10 (C#)
* Architecture: Microservices, Clean Architecture, Domain-Driven Design (DDD)
* Patterns: CQRS (MediatR), Result Pattern (ErrorOr for exceptionless error handling)
* Testing: xUnit, FluentAssertions, NSubstitute

## Repository Structure

The project is divided into the following main areas:

* /src/Services: Contains independent microservices (Ordering, Catalog, Users, Analytics).
* /src/ApiGateways: Entry points for the client applications to interact with the microservices.
* /src/Client: Frontend applications (Web/Mobile).

## Prerequisites

To run the services locally, you will need to install the following tools:

1. .NET 10 SDK
2. Docker Desktop or Docker Engine
3. An IDE such as Visual Studio Code or Visual Studio 2022
4. Optional: Postman or a similar tool to test HTTP requests

## Initial Setup

### 1. Environment Variables
Before running the application, create a .env file in the root directory based on the .env.example file:

* Copy the example file: cp .env.example .env
* Update DB_PASSWORD with your desired local SQL Server password.

### 2. Database Infrastructure
The project uses SQL Server via Docker. To start the database:

1. Ensure Docker is running.
2. Run the following command in the root directory:
   docker-compose up -d

### 3. Database Migrations
Once the database container is healthy, apply the migrations to create the schemas and tables:

dotnet ef database update --project src/Services/Ordering/Ordering.Infrastructure --startup-project src/Services/Ordering/Ordering.API

## How to Run the Project Locally

Follow these steps to run the active services:

1. Restore the solution dependencies:
   dotnet restore

2. Navigate to the Ordering API and run the service:
   cd src/Services/Ordering/Ordering.API
   dotnet run

## API Documentation

The project includes automatically generated interactive documentation. Once the Ordering service is running, the terminal will display the local URL (e.g., https://localhost:5001).

To view the endpoints, request schemas, and possible error codes, open your web browser and navigate to:
https://localhost:<PORT>/swagger

## 📂 Folder Structure

The repository follows a clear separation of concerns:

POS/
├── src/
│   ├── ApiGateways/      # Gateway routing for client applications
│   ├── Client/           # Frontend applications (Web UI, Mobile)
│   └── Services/         # Autonomous backend microservices
│       ├── Analytics/    # Future: Business intelligence and reporting
│       ├── Catalog/      # Future: Product and inventory management
│       ├── Users/        # Future: Identity and access management
│       └── Ordering/     # Active: Core business logic for order processing
│           ├── Ordering.API/
│           ├── Ordering.Application/
│           ├── Ordering.Domain/
│           ├── Ordering.Infrastructure/
│           └── Ordering.UnitTests/
├── deploy/               # Future: CI/CD pipelines, Kubernetes manifests
├── docs/                 # General project documentation
├── docker-compose.yml    # Infrastructure orchestration
├── .env.example          # Environment variables template
└── POS.sln               # Solution file