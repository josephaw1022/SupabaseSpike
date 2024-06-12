var builder = DistributedApplication.CreateBuilder(args);



var existingSupabaseDb = builder.AddConnectionString("Supabase");



var dbMigration = builder.AddProject<Projects.SupabaseSpike_DatabaseMigrations>("dbmigration")
    .WithReference(existingSupabaseDb);


builder.AddProject<Projects.SupabaseSpike_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(existingSupabaseDb);

builder.Build().Run();
