import { User } from './User';

export interface PhotoLike {
    userId: number;
    photoId: number;
    liker: User;
}