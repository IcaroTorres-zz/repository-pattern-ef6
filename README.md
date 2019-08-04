## Disclaimer
- This template was planned depending on Dependency Injection pattern;
 
- This Unit of Work is:
	> A centralizing implementation, able to manage many DbContexts for it's repositories instances;
	> It's repositories can be accessed through generic getter with desired Entity as type parameter;
	> Their DbContexts can be accessed through generic getter with their type as type parameter.

- The Repository can be used as:
	> Base implementation inherited to specific each repositories;
	> Generic Repository based in type parameters directly used in code.