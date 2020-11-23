import { User } from './user';

export interface PhotoLike {
    userId: number;
    photoId: number;
    userPhotoUrl: string;
    username: string;
    name: string;
    surname: string;
}