import { Component, inject, OnInit, signal } from '@angular/core'
import { CommonModule } from '@angular/common'
import { FormsModule } from '@angular/forms'
import { TodoService } from './services/todo.service'
import { TodoItem } from './models/todo.model'

@Component({
	selector: 'app-root',
	imports: [CommonModule, FormsModule],
	templateUrl: './app.html',
	styleUrl: './app.css',
})
export class App implements OnInit {
	private readonly todoService = inject(TodoService)

	protected readonly todos = signal<TodoItem[]>([])
	protected readonly newTodoTitle = signal('')
	protected readonly isLoading = signal(false)
	protected readonly error = signal<string | null>(null)

	ngOnInit(): void {
		this.loadTodos()
	}

	protected loadTodos(): void {
		this.isLoading.set(true)
		this.error.set(null)

		this.todoService.getTodos().subscribe({
			next: todos => {
				this.todos.set(todos)
				this.isLoading.set(false)
			},
			error: err => {
				this.error.set('Failed to load todos')
				this.isLoading.set(false)
				console.error('Error loading todos:', err)
			},
		})
	}

	protected addTodo(): void {
		const title = this.newTodoTitle().trim()
		if (!title) return

		this.isLoading.set(true)
		this.error.set(null)

		this.todoService.addTodo(title).subscribe({
			next: todo => {
				this.todos.update(todos => [...todos, todo])
				this.newTodoTitle.set('')
				this.isLoading.set(false)
			},
			error: err => {
				this.error.set('Failed to add todo')
				this.isLoading.set(false)
				console.error('Error adding todo:', err)
			},
		})
	}

	protected toggleTodo(todo: TodoItem): void {
		this.todoService.toggleTodo(todo.id, !todo.isCompleted).subscribe({
			next: updatedTodo => {
				this.todos.update(todos => todos.map(t => (t.id === updatedTodo.id ? updatedTodo : t)))
			},
			error: err => {
				this.error.set('Failed to update todo')
				console.error('Error updating todo:', err)
			},
		})
	}

	protected deleteTodo(id: string): void {
		this.todoService.deleteTodo(id).subscribe({
			next: () => {
				this.todos.update(todos => todos.filter(t => t.id !== id))
			},
			error: err => {
				this.error.set('Failed to delete todo')
				console.error('Error deleting todo:', err)
			},
		})
	}

	protected get completedCount(): number {
		return this.todos().filter(t => t.isCompleted).length
	}

	protected get remainingCount(): number {
		return this.todos().filter(t => !t.isCompleted).length
	}
}
