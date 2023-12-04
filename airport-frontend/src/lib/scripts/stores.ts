import { readable } from 'svelte/store';
import {getAuthorized} from './api'

const authorized = readable(false, (set) => {
    // Call getAuthorized and then set the value of the store
    getAuthorized().then(set);

    // Set up an interval to update the authorized status every 10 seconds
    const interval = setInterval(() => {
        getAuthorized().then(set);
    }, 10000);

    // Return a cleanup function to clear the interval
    return () => clearInterval(interval);
})