```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.7462/25H2/2025Update/HudsonValley2)
12th Gen Intel Core i5-12450H 2.00GHz, 1 CPU, 12 logical and 8 physical cores
  [Host]     : .NET Framework 4.8.1 (4.8.9221.0), X86 LegacyJIT
  DefaultJob : .NET Framework 4.8.1 (4.8.9221.0), X86 LegacyJIT


```
| Method     | Mean       | Error     | StdDev    | Gen0   | Gen1   | Allocated |
|----------- |-----------:|----------:|----------:|-------:|-------:|----------:|
| Aproximace |   1.020 μs | 0.0054 μs | 0.0048 μs | 0.2365 |      - |    1242 B |
| HeldKarp   |  42.188 μs | 0.2501 μs | 0.2339 μs | 7.1411 | 0.7324 |   37529 B |
| HrubaSila  | 640.770 μs | 2.4717 μs | 2.3121 μs |      - |      - |     680 B |
