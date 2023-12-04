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

export async function getAuthorized(): Promise<boolean> {
    const response = await fetch(`/api/auth/check-authorized`);
    if (response.ok) {
        return true
    }
    else if (response.status in [401, 403]){
        return false
    }
    else {
        throw new Error('Failed to fetch departing flights');
    }
}


// Example usage:
// getDepartingFlights('JFK')
//   .then(flights => console.log(flights))
//   .catch(error => console.error(error));