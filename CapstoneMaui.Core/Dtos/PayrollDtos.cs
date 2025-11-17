namespace CapstoneMaui.Core.Dtos;

public record PayStubDto(string period, decimal gross, decimal net);
public record PayEstimeDto(string periodToDate, decimal estimatedGross);