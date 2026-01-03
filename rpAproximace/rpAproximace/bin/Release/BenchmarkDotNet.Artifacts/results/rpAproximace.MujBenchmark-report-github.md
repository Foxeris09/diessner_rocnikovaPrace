```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.7462/25H2/2025Update/HudsonValley2)
12th Gen Intel Core i5-12450H 2.00GHz, 1 CPU, 12 logical and 8 physical cores
  [Host]     : .NET Framework 4.8.1 (4.8.9221.0), X86 LegacyJIT
  DefaultJob : .NET Framework 4.8.1 (4.8.9221.0), X86 LegacyJIT


```
| Method     | Mean         | Error        | StdDev       | Gen0   | Gen1   | Allocated |
|----------- |-------------:|-------------:|-------------:|-------:|-------:|----------:|
| Aproximace |     989.6 ns |      6.78 ns |      5.66 ns | 0.2384 |      - |    1254 B |
| HeldKarp   |  46,206.4 ns |    918.42 ns |  1,429.86 ns | 7.1411 | 0.7324 |   37529 B |
| HrubaSila  | 671,671.3 ns | 11,492.01 ns | 10,187.37 ns |      - |      - |     672 B |
