# Food recipes app using .NET MAUI and ASP.NET Core
**Recipe Cabinet** is a cross-platform mobile application developed using .NET MAUI, designed to help users discover, save, manage, and share culinary recipes. It also provides nutritional information and allows users to rate their favorite recipes. The backend is powered by ASP .NET Core Web API and PostgreSQL.

**Note**: This application was developed as part of my Bachelor's degree project at Transilvania University of Brașov.

## Project structure
```
repo
|__RecipeCabinet -> maui project
|__RecipeCabinetAPI
|          |__RecipeCabinet.Tests -> xUnit tests project
|          |__RecipeCabinetAPI -> web api project
|__.gitignore
|__README.md
```

## Features

- **Explore Recipes**: Discover new recipes using various filters like recipe name, ingredients, rating, and cook time.
- **Nutrition Calculation**: Maintain a healthy lifestyle by accessing detailed nutritional information for saved recipes and custom ingredients.
- **User Management**: Register, log in, change password, and manage your account details.
- **Recipe Management**: Create, edit, delete, and rate recipes. Save recipes locally for offline access.
- **Community Interaction**: Share recipes and rate them.

## Setup and Installation

### Prerequisites

- .NET 7 SDK or later
- Visual Studio 2022 or later with .NET MAUI workload
- PostgreSQL
- AWS account for S3 bucket
- Edamam API credentials

### Steps

1. **Clone the repository**:
   ```
   git clone https://github.com/EneClaudiu/food-recipes-maui-app.git
   ```
2. **Setup the database**:
   
   Create a PostgreSQL database with the three tables corresponding to the `Recipe`, `User` and `RecipeRatings` models from `RecipeCabinetAPI`.
3. **Configure AWS S3**:
   
   Create a bucket for storing images and an IAM User.
   
4. **Create Edamam API account**:
   
   Create an [Edamam Nutrition Analysis API](https://developer.edamam.com/edamam-nutrition-api) account.
5. **Setup the API settings**:

   In the `RecipeCabinetAPI/appsettings.json`, configure the Database and AWS.
   
   Also, choose a JWT key.
6. **Configure the MAUI project**:
   
   In `RecipeCabinet/MauiProgram.cs` replace the API Uri with yours and in `RecipeCabinet/Services/EdamamService.cs` enter the app credentials from your Edamam account.

7. **Run the project**:

   Run the API first, then the MAUI app.

   `Note: Depending on the system configurations, the first API call may take longer than expected.`

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
