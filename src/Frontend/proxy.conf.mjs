const backendUrl = process.env.services__webapi__http__0 || 'http://localhost:5273'

console.log(`[Proxy] Using backend URL: ${backendUrl}`)

export default {
	'/api': {
		target: backendUrl,
		secure: false,
		changeOrigin: true,
		logLevel: 'debug',
		pathRewrite: {
			'^/api': '',
		},
	},
}
