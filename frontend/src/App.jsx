import { useEffect, useState } from 'react'
import { createTask, deleteTask, getTasks, updateTask } from './api/tasksApi'
import TaskForm from './components/TaskForm'
import TaskList from './components/TaskList'

function App() {
  const [tasks, setTasks] = useState([])
  const [isLoading, setIsLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    getTasks()
      .then(setTasks)
      .catch((err) => setError(`Could not load tasks: ${err.message}. Is the backend running?`))
      .finally(() => setIsLoading(false))
  }, [])

  // Each handler returns true on success so child components can reset their own state.
  async function handleAdd(task) {
    try {
      const created = await createTask(task)
      setTasks((current) => [...current, created])
      setError('')
      return true
    } catch (err) {
      setError(err.message)
      return false
    }
  }

  async function handleUpdate(id, task) {
    try {
      const updated = await updateTask(id, task)
      setTasks((current) => current.map((t) => (t.id === id ? updated : t)))
      setError('')
      return true
    } catch (err) {
      setError(err.message)
      return false
    }
  }

  async function handleDelete(id) {
    try {
      await deleteTask(id)
      setTasks((current) => current.filter((t) => t.id !== id))
      setError('')
    } catch (err) {
      setError(err.message)
    }
  }

  const completedCount = tasks.filter((t) => t.isCompleted).length

  return (
    <main className="app">
      <h1>Task Manager</h1>
      <TaskForm onAdd={handleAdd} />
      {error && <p className="error">{error}</p>}
      {isLoading ? (
        <p>Loading tasks...</p>
      ) : (
        <>
          <p className="summary">
            {completedCount} of {tasks.length} completed
          </p>
          <TaskList tasks={tasks} onUpdate={handleUpdate} onDelete={handleDelete} />
        </>
      )}
    </main>
  )
}

export default App
