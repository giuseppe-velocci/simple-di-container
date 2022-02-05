# simple-di-container
A very basic DI container in dotnet framework (just for fun XD)

This DI container uses auto wiring via Reflection. This computation is performed only once at registration time.

It also allows two lifestyles for resolved instances:
 - *Transient*: a different instance is created each time that Type is required
 - *Singleton*: always the same instance is reaturned each time that Type is required
