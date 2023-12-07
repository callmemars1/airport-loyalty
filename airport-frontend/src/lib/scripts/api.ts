import type {TableFlightDto} from './dto';

export async function getDepartingFlights(atAirportIataCode: string, fromUtc?: string): Promise<TableFlightDto[]> {
    const queryParams = new URLSearchParams();
    queryParams.append('atAirportIataCode', atAirportIataCode);
    if (fromUtc) queryParams.append('fromUtc', fromUtc);

    const response = await fetch(`/api/flights/departing?${queryParams}`);
    if (!response.ok) {
        throw new Error('Failed to fetch departing flights');
    }

    return response.json();
}

export async function getArrivingFlights(atAirportIataCode: string, fromUtc?: string): Promise<TableFlightDto[]> {
    const queryParams = new URLSearchParams();
    queryParams.append('atAirportIataCode', atAirportIataCode);
    if (fromUtc) queryParams.append('fromUtc', fromUtc);

    const response = await fetch(`/api/flights/arriving?${queryParams}`);
    if (!response.ok) {
        throw new Error('Failed to fetch departing flights');
    }

    return response.json();
}