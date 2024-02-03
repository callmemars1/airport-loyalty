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

export interface UserDto {
    id: string; // Assuming GUIDs are represented as strings in TypeScript
    name: string;
    surname: string;
    patronymic: string | null; // or undefined, depending on your use case
    passportNumber: string;
    createdAt: Date; // or string, if you prefer to work with date as ISO string
    login: string;
}