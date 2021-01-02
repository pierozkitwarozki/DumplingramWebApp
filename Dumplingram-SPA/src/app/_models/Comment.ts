import { Content } from "@angular/compiler/src/render3/r3_ast";

export interface PhotoComment {
    id: number;
    commenterId: number;
    commenterUsername: string;
    content: string;
}
