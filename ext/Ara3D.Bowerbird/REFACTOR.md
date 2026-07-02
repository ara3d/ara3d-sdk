I want you to refactor Ara3D.Bowerbird (and Ara3D.Bowerbird.Revit2025 and Ara3D.Bowerbird.Demo but not the Ara3D.Bowerbird.RevitSamples) so that:
    - Each command live in its own folder
    - In a folder maybe zero or more files  
    - One DLL will be produced per folder
    - Compilation happens on demand when the command is launched
    - A JSON manifest  
    - Every command is required to have its own JSON manifest file
        - named COMMAND_NAME.manifest.json
        - contains 
            - Short UI friendly name
            - TypeName (with namespace)
            - Optional: short description
            - Optional: long description
    - Gerate compilation logs containing warnings / diagnostics / errors 
    - DLLs / Compilation logs aren't pushed to Git 

I want you create a new console project which will be used for demonstrations and testing: 
- Ara3D.Bowerbird.Console

I want you to create some sample commands that work with the console version 
- Ara3D.Bowerbird.TestSamples

Potential future plans to be aware of:
- We may add cahcing so recompilation only happens if the source files or the library version changes
- Tool groups per folder, where a manifest might contain multiple file
- We may add custom libraries per folder 
- We may have different UI modalities (e.g. data grid)
- We will use the system for compiling MCP tools  
- We may want to compile a folder containing only helper code which is referenced or used by other tools. 
- An Automated versioning system 

Any opportunities to make the code more modular and easier to refactor should be taken. 

Helper code should be centralized. 

Keep the code simple and easy to undertand. 

Set clear achievable goals.