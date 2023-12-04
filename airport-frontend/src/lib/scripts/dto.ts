export interface AirportDto {
    City: string;
    Code: string;
}

export interface GateDto {
    Terminal: string;
    Name: string;
}

export interface AirlineDto {
    Title: string;
    Code: string;
}

export interface TableFlightDto {
    ArrivalAirport: AirportDto;
    ArrivalGate: GateDto;
    ArrivalDateTime: string; // DateTimes are represented as strings in JSON
    DepartureAirport: AirportDto;
    DepartureGate: GateDto;
    DepartureDateTime: string;
    Airline: AirlineDto;
    FlightNumber: string;
}
