# Meter Change API

[![License](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
![.NET](https://github.com/pikkauhn/MeterChangeApi/actions/workflows/dotnet.yml/badge.svg)

This API provides functionality for managing water meter change-out operations. It allows you to interact with data related to endpoints, meters, addresses, and ArcGIS information. User authentication and authorization are also included.

## Table of Contents

* [Introduction](#introduction)
* [Features](#features)
* [Getting Started](#getting-started)
    * [Prerequisites](#prerequisites)
    * [Installation](#installation)
    * [Configuration](#configuration)
    * [Running the API](#running-the-api)
* [API Endpoints](#api-endpoints)
    * [Authentication](#authentication)
    * [Addresses](#addresses)
    * [ArcGIS Data](#arcgis-data)
    * [Endpoints](#endpoints)
    * [Meters](#meters)
* [Environment Variables](#environment-variables)
* [Contributing](#contributing)
* [License](#license)
* [Contact](#contact)

## Introduction

The Meter Change API is built using ASP.NET Core and Entity Framework Core. It aims to streamline the process of managing water meter replacements by providing a centralized system for accessing and manipulating relevant data.

## Features

* **Endpoint Management:** Create, read, update, and delete water endpoint information.
* **Meter Management:** Create, read, update, and delete water meter details.
* **Address Management:** Create, read, update, delete, and query address data (by ID, paginated, all, range, street, location ICN, building status). Includes JSON export.
* **ArcGIS Data Management:** Create, read, update, and delete ArcGIS related data.
* **User Authentication:** Secure the API using JWT authentication with user registration and token generation.
* **Pagination:** Efficiently retrieve large datasets using paginated endpoints.
* **Error Handling:** Robust exception handling middleware for clear error responses.
* **Database Operations Handling:** Consistent handling of database interactions with error logging.

## Getting Started

Follow these instructions to get the API up and running on your local machine.

### Prerequisites

* [.NET SDK](https://dotnet.microsoft.com/download) (version compatible with the project - check `global.json` if present)
* [SQL Server](https://www.microsoft.com/sql-server/sql-server-downloads) (or another database supported by Entity Framework Core, with appropriate connection string configuration)
* [Git](https://git-scm.com/) (for cloning the repository)
* (Optional) [Visual Studio](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/) with C# extension for development.

### Installation

1.  **Clone the repository:**
    ```bash
    git clone [https://github.com/](https://github.com/)[YOUR_GITHUB_USERNAME]/MeterChangeApi.git
    cd MeterChangeApi
    ```

2.  **Restore NuGet packages:**
    ```bash
    dotnet restore
    ```

### Configuration

1.  **Database Connection:**
    * Navigate to the `appsettings.json` file in the project root.
    * Update the `ConnectionStrings:DefaultConnection` value with your SQL Server connection string. Ensure the server name, database name, user ID, and password are correct.

    ```json
    {
      "ConnectionStrings": {
        "DefaultConnection": "Server=your_server;Database=MeterChangeDb;User Id=your_user;Password=your_password;TrustServerCertificate=True"
      },
      // ... other configurations
    }
    ```

2.  **JWT Configuration:**
    * In the `appsettings.json` file, configure the JWT settings under the `Jwt` section.
    * **`Key`**: A secret key used for signing the JWT tokens. **Ensure this is a strong, unique value for production environments.**
    * **`Issuer`**: The issuer of the JWT tokens (your application URL or name).
    * **`Audience`**: The intended audience of the JWT tokens (the client applications that will consume the API).
    * **`TokenLifetimeInMinutes`**: The duration for which the generated tokens will be valid.

    ```json
    {
      // ... other configurations
      "Jwt": {
        "Key": "your_super_secret_key_here",
        "Issuer": "https://localhost:yourport",
        "Audience": "[https://yourclientapp.com](https://yourclientapp.com)",
        "TokenLifetimeInMinutes": 60
      }
    }
    ```

3.  **(Optional) Environment Variables:**
    * You can also configure these settings using environment variables. For example:
        * `ConnectionStrings__DefaultConnection`: Your connection string.
        * `Jwt__Key`: Your JWT secret key.
        * `Jwt__Issuer`: Your JWT issuer.
        * `Jwt__Audience`: Your JWT audience.
        * `Jwt__TokenLifetimeInMinutes`: Token lifetime in minutes.

### Running the API

1.  **Apply database migrations:**
    ```bash
    dotnet ef database update
    ```
    This command will create the database and apply any pending migrations based on your Entity Framework Core models and configuration.

2.  **Build and run the API:**
    ```bash
    dotnet run
    ```

    The API should now be running on the port specified in your `launchSettings.json` file (usually `https://localhost:xxxx`).

## API Endpoints

Below is a summary of the available API endpoints. Refer to the controller files for more detailed information on request bodies and response structures.

### Authentication

* `POST /api/Auth/register`: Registers a new user. Requires `username`, `password`, and optionally `email` in the request body.
* `POST /api/Auth/generate-token`: Generates a JWT token for an existing user. Requires a JSON request body with `username` and `password`.

### Addresses

* `GET /api/Addresses/{id}`: Retrieves an address by its ID.
* `GET /api/Addresses?pageNumber={pageNumber}&pageSize={pageSize}`: Retrieves a paginated list of addresses.
* `GET /api/Addresses/export-json`: Exports all addresses to a JSON file.
* `POST /api/Addresses`: Creates a new address. Requires address data in the request body.
* `PUT /api/Addresses/{id}`: Updates an existing address. Requires the address ID in the URL and updated address data in the request body.
* `DELETE /api/Addresses/{id}`: Deletes an address by its ID.
* `GET /api/Addresses/getalladdresses`: Retrieves all addresses.
* `GET /api/Addresses/range?x={x}&y={y}&distanceInFeet={distanceInFeet}`: Retrieves addresses within a specified geographical range.
* `GET /api/Addresses/street/{street}`: Retrieves addresses on a specific street.
* `GET /api/Addresses/locationicn/{locationIcn}`: Retrieves an address by its location ICN.
* `GET /api/Addresses/buildingstatus/{buildingStatus}`: Retrieves addresses with a specific building status.

### ArcGIS Data

* `GET /api/ArcGISData/{id}`: Retrieves ArcGIS data by its ID.
* `GET /api/ArcGISData/getallarcgisdata`: Retrieves all ArcGIS data.
* `GET /api/ArcGISData?pageNumber={pageNumber}&pageSize={pageSize}`: Retrieves paginated ArcGIS data.
* `POST /api/ArcGISData`: Creates new ArcGIS data.
* `PUT /api/ArcGISData/{id}`: Updates existing ArcGIS data.
* `DELETE /api/ArcGISData/{id}`: Deletes ArcGIS data by ID.

### Endpoints

* `GET /api/Endpoints/{id}`: Retrieves an endpoint by its ID.
* `GET /api/Endpoints/getallendpoints`: Retrieves all endpoints.
* `GET /api/Endpoints?pageNumber={pageNumber}&pageSize={pageSize}`: Retrieves paginated endpoints.
* `POST /api/Endpoints`: Creates a new endpoint.
* `PUT /api/Endpoints/{id}`: Updates an existing endpoint.
* `DELETE /api/Endpoints/{id}`: Deletes an endpoint by its ID.

### Meters

* `GET /api/Meters/{id}`: Retrieves a meter by its ID.
* `GET /api/Meters/getallmeters`: Retrieves all meters.
* `GET /api/Meters?pageNumber={pageNumber}&pageSize={pageSize}`: Retrieves paginated meters.
* `POST /api/Meters`: Adds a new meter.
* `PUT /api/Meters/{id}`: Updates an existing meter.
* `DELETE /api/Meters/{id}`: Deletes a meter by its ID.

## Environment Variables

While configuration can be done in `appsettings.json`, using environment variables is recommended for sensitive information and production deployments. The API will automatically read configuration values from environment variables if they are set. See the [Configuration](#configuration) section for the naming convention of environment variables.

## Contributing

Contributions to this API are welcome. Please follow these steps:

1.  Fork the repository.
2.  Create a new branch for your feature or bug fix.
3.  Make your changes and ensure they are well-tested.
4.  Submit a pull request with a clear description of your changes.

## License

This API is licensed under the [MIT License](LICENSE).

## Contact

For any questions or issues, please feel free to [open an issue](https://github.com/[YOUR_GITHUB_USERNAME]/MeterChangeApi/issues).

---