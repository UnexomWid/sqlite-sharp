# About <a href="https://docs.microsoft.com/en-us/dotnet/framework/whats-new/#v45"><img align="right" src="https://img.shields.io/badge/.Net%20Framework-4.5-5C2D91?logo=.net" alt=".Net Framework 4.5" /></a>

This is a very simple C# wrapper for [SQLite 3](https://www.sqlite.org/). Only the essential API functions are supported (i.e. creating statements, binding
parameters, executing statements).

# Usage

Here's an example:

```cs
// Initialize the database
SQLite.DB db = new SQLite.DB("database.sqlite");

// Create the statement
SQLite.DB.Statement stmt = db.CreateStatement("SELECT * FROM dummy_table WHERE id = ?");

// Bind the value 4 to the first parameter
stmt.BindInt(1, 4);

// Execute the statement and receive an array of columns.
// You can execute a statement multiple times, and bind
// different values on the same parameter.
SQLite.Column[] result = stmt.Exec();

if(result != null) {
    // If it's null, query didn't return anything. Otherwise, there is at least one row.
    
    for (int i = 0; i < result.Length; ++i)
    {
        // Column name
        string name = result[i].name;
        
        // Column type
        SQLite.Column.Type type = result[i].type;
        
        // Column values. Make sure to cast those to the correct type:
        // INTEGER -> long
        // FLOAT   -> double
        // TEXT    -> string
        // BLOB    ->byte[]
        List<object> values = result[i].values;
        
        // Every column will have the same number of values,
        // which is also the number of rows.
    }
}
```

# License  <a href="https://github.com/UnexomWid/sqlite-sharp/blob/master/LICENSE"><img align="right" src="https://img.shields.io/badge/License-MIT-blue.svg" alt="License: MIT" /></a>

This wrapper was created by [UnexomWid](https://uw.exom.dev). It is licensed under the [MIT](https://github.com/UnexomWid/sqlite-sharp/blob/master/LICENSE) license.