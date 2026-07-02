# Coding Guidelines 

- Code must be:
  1. correct
  2. concise
  3. simple
  4. resuable
  5. performant
  
# General Principles

- Start with the simplest thing that could possibly work
  - Note areas that need validation or deeper consideration as to-do statements
  - Once that works consider areas for improvement
- Keep code short and to the point
  - Humand and AI agents have a limited context windows  
  - Shorter code is easier to read, understand, and validate
  - Minimize the number of statements
  - Prefer expressions to statements
  - Break up large functions, classes, and projects    
  - Place assertions to document assumptions, and test preconditions, postconditions
  - Write as little code as possible
- Simplify code
  - It should be hard to use code wrongly
  - Minimize conditions for calling the code
  - Code should be generous in the data it accepts and conservative
  - Try to avoid side-effects
  - Minimize implicit coupling    
  - Functions, classes, and libraries should do one thing and do it well
  - They need to have a well-defined responsibility or task
  - Prefer to create new functions, classes, interfaces, or libraries, instead of overloading existing ones with new responsibilities
  - They should be easy to explain, without too much caveats 
- Aim to maximize reusability
  -  Code tends to be well-engineered and easier to validate and test
  -  This is because it is easier to refactor, move, and fix
- Don't repeat yourself
   - Repetition is a form of dependency
   - It is usually a missed opportuntity for refactoring  
- Write code as if writing a library
- Well engineered code isn't significantly more effort
- Keep things simple
- Refactor code early and often  
- Put reusable helper code in separate files and/or libraries  
- Think in terms of data flow  
- Minimize side effects
- Prefer immutable structs
- Lambdas are a great way to make code generic and reusable in a wide variety of context   
- Minimize coupling
- Prefer functional code
- Prefer interface to classes
- Interfaces should represent immutable concepts 
- Prefer immutable classes to mutable classes   

# Avoid 

- Avoid premature optimization
- Avoid over-engineering
- Be generous in what you accept and conservative in what you return 
- If a sequence of steps could be useful in another context
- Interfaces should be as small as possible to cover minimum required behavior

# Rules of Thumb

- Break projects up into smaller libraries when it starts to make sense
- Break up large functions early  
- Prefer static methods 
- Prefer extension methods  
- Deeply nested method chains (e.g. A().B().C().D()) should be refactored into functions
- Keep function signature small
- Use structs instead when data neeeds to be moved around together
- Document areas for improvement
- Document design decisions made, when there are multiple viable options
- Document things that are unsure and should be validated (e.g. possible performance problems)

# C# Specific 

- Use implicitly typed variables ('var') as much as possible
- Prefer `IReadOnlyList`  over List, or Array, as an argument 
- Prefer expression body methods
