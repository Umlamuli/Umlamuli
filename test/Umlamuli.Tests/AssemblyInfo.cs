// ServiceRegistrar holds registration limits in static fields, so parallel
// AddUmlamuli calls across test classes race on shared state. Serialize all
// tests in this assembly until the statics are removed.
[assembly: Xunit.CollectionBehavior(DisableTestParallelization = true)]
