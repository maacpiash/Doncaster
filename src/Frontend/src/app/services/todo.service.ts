import { HttpClient } from '@angular/common/http'
import { Injectable, inject } from '@angular/core'
import { Observable } from 'rxjs'
import { TodoItem } from '../models/todo.model'

@Injectable({
	providedIn: 'root',
})
export class TodoService {
	private readonly http = inject(HttpClient)
	private readonly apiUrl = '/api'

	getTodos(): Observable<TodoItem[]> {
		return this.http.get<TodoItem[]>(this.apiUrl)
	}

	addTodo(title: string): Observable<TodoItem> {
		return this.http.post<TodoItem>(this.apiUrl, { title })
	}

	toggleTodo(id: string, isCompleted: boolean): Observable<TodoItem> {
		return this.http.patch<TodoItem>(`${this.apiUrl}/${id}`, { isCompleted })
	}

	deleteTodo(id: string): Observable<void> {
		return this.http.delete<void>(`${this.apiUrl}/${id}`)
	}
}
