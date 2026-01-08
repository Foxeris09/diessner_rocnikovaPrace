```

BenchmarkDotNet v0.13.12, Windows 11 (10.0.26200.7462)
12th Gen Intel Core i5-12450H, 1 CPU, 12 logical and 8 physical cores
.NET SDK 9.0.103
  [Host]   : .NET 8.0.13 (8.0.1325.6609), X64 RyuJIT AVX2
  .NET 8.0 : .NET 8.0.13 (8.0.1325.6609), X64 RyuJIT AVX2

Job=.NET 8.0  Runtime=.NET 8.0  

```
| Method     | Mean           | Error          | StdDev         | Gen0      | Gen1      | Gen2      | Allocated    |
|----------- |---------------:|---------------:|---------------:|----------:|----------:|----------:|-------------:|
| Aproximace |       2.517 μs |      0.0665 μs |      0.1908 μs |    0.5035 |    0.0038 |         - |      3.09 KB |
| HeldKarp   | 644,192.955 μs | 12,296.9782 μs | 25,119.4678 μs | 1000.0000 | 1000.0000 | 1000.0000 | 163841.59 KB |
