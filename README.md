# Survey Project

## Overview
This project is a survey application built using C# for the backend and Blazor Razor for the frontend. It allows users to create, manage, and participate in surveys. 

![Survey Application](https://via.placeholder.com/800x400.png?text=Survey+Application)

## Technologies Used
- **Backend:** C#
- **Frontend:** Blazor Razor
- **Database:** Entity Framework Core

## Project Structure
- **Components/**: Contains reusable UI components.
- **Data/**: Contains the database context and data models.
- **DTOs/**: Data Transfer Objects for communication between client and server.
- **Entities/**: Contains the entity models used in the application.
- **Helpers/**: Utility classes and methods.
- **Mappers/**: Mapping classes for converting between entities and DTOs.
- **Migrations/**: Database migrations for schema changes.
- **Repositories/**: Data access layer for interacting with the database.
- **Seeders/**: Classes for seeding initial data into the database.
- **Services/**: Business logic layer for handling application operations.
- **wwwroot/**: Static files such as CSS and JavaScript.

## Getting Started
1. Clone the repository:
   ```bash
   git clone <repository-url>
   cd Survey
   ```
2. Restore the dependencies:
   ```bash
   dotnet restore
   ```
3. Run the application:
   ```bash
   dotnet run
   ```

## Usage
- Navigate to the application in your web browser to access the survey functionalities.

## Contributing
Contributions are welcome! Please open an issue or submit a pull request for any improvements or bug fixes.

## License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
