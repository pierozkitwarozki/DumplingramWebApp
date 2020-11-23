import { Photo } from './photo';

export interface User {
    id: number;
    username: string;
    age: number;
    name: string;
    description: string;
    surname: string;
    city: string;
    country: string;
    photoUrl: string;
    token: string;
}
