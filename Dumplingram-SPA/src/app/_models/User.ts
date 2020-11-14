import { Photo } from './Photo';

export interface User {
    id: number;
    username: string;
    gender?: string;
    age: number;
    name: string;
    description: string;
    surname: string;
    city: string;
    country: string;
    photoUrl: string;
    photos?: Photo[];
    token: string;
}
